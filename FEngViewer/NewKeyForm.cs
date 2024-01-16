using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using FEngLib.Scripts;
using JetBrains.Annotations;

namespace FEngViewer
{
    public partial class NewKeyForm : Form
    {
        record KeyOption(string label, int autoTime, object value)
        {
            [UsedImplicitly]
            public string Display => $"{label}: {value}";
        }

        private Track _track;

        public object NewKeyValue => ((KeyOption)dupeKeyComboBox.SelectedItem).value;
        public int NewKeyTime => keyTimeTrackBar.Value;

        public NewKeyForm(Track track)
        {
            _track = track;
            InitializeComponent();
            ConfigureKeyComboBox();
            ConfigureTimeTrackBar();
            FillKeyComboBox();
        }

        private void ConfigureKeyComboBox()
        {
            dupeKeyComboBox.DisplayMember = "Display";
            dupeKeyComboBox.SelectedIndexChanged += DupeKeyComboBoxOnSelectedIndexChanged;
        }

        private void ValidateForm()
        {
            okButton.Enabled = dupeKeyComboBox.SelectedItem is not null &&
                               !KeyExistsAtTime(_track, keyTimeTrackBar.Value);
        }

        private void DupeKeyComboBoxOnSelectedIndexChanged(object sender, EventArgs e)
        {
            if (keyTimeTrackBar.Value == 0)
            {
                keyTimeTrackBar.Value = ((KeyOption)dupeKeyComboBox.SelectedItem).autoTime;
            }
            ValidateForm();
        }

        private void ConfigureTimeTrackBar()
        {
            keyTimeTrackBar.Minimum = 0;
            keyTimeTrackBar.Maximum = (int)_track.Length;
            keyTimeTrackBar.Value = (int) _track.Length;
            keyTimeTrackBar.ValueChanged += KeyTimeTrackBarOnValueChanged;
            keyTimeTrackBar.Value = 0;
        }

        private void KeyTimeTrackBarOnValueChanged(object sender, EventArgs e)
        {
            ValidateForm();
            keyTimestampLabel.Text = $"{keyTimeTrackBar.Value}ms";
        }

        private void FillKeyComboBox()
        {
            switch (_track)
            {
                case ColorTrack colorTrack:
                    FillKeyComboBoxInternal(colorTrack);
                    break;
                case Vector2Track vector2Track:
                    FillKeyComboBoxInternal(vector2Track);
                    break;
                case Vector3Track vector3Track:
                    FillKeyComboBoxInternal(vector3Track);
                    break;
                case QuaternionTrack quaternionTrack:
                    FillKeyComboBoxInternal(quaternionTrack);
                    break;
                default:
                    throw new Exception($"Unsupported track type: {_track.GetType()}");
            }
        }

        private static bool KeyExistsAtTime(Track track, int time)
        {
            return track switch
            {
                ColorTrack colorTrack => colorTrack.DeltaKeys.Any(dk => dk.Time == time),
                Vector2Track vector2Track => vector2Track.DeltaKeys.Any(dk => dk.Time == time),
                Vector3Track vector3Track => vector3Track.DeltaKeys.Any(dk => dk.Time == time),
                QuaternionTrack quaternionTrack => quaternionTrack.DeltaKeys.Any(dk => dk.Time == time),
                _ => throw new Exception($"Unsupported track type: {track.GetType()}")
            };
        }

        private void okButton_Click(object sender, EventArgs e)
        {
            DialogResult = DialogResult.OK;
            Close();
        }

        private void FillKeyComboBoxInternal<TTrackValue>(ITrack<TTrackValue> track) where TTrackValue : struct
        {
            var options = new object[track.DeltaKeys.Count + 1];
            options[0] = new KeyOption("Base Key", 0, track.BaseKey);
            var index = 1;

            for (var deltaKeyNode = track.DeltaKeys.First;
                 deltaKeyNode != null;
                 deltaKeyNode = deltaKeyNode.Next)
            {
                int autoTime;
                if (deltaKeyNode.Previous is { } previous)
                    autoTime = (deltaKeyNode.Value.Time + previous.Value.Time) / 2;
                else
                    autoTime = deltaKeyNode.Value.Time + 1;

                options[index++] = new KeyOption($"T = {deltaKeyNode.Value.Time}ms",
                    autoTime,
                    TrackHelpers.AddKeys(track.BaseKey, deltaKeyNode.Value.Val));
            }

            dupeKeyComboBox.Items.AddRange(options);
            dupeKeyComboBox.SelectedIndex = 0;
        }
    }
}

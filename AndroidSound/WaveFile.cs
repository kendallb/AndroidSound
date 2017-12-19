/*
 * Copyright (C) 2004-2017 AMain.com, Inc.
 * All Rights Reserved
 */

using System;
using System.IO;
using System.Threading.Tasks;
using Android.Media;

namespace AndroidSound
{
    /// <summary>
    /// class to gather up all the common functions used by all client applications
    /// </summary>
    public class WaveFile : IDisposable
    {
        private readonly AudioTrack _audioTrack;
        private readonly byte[] _soundBuffer;
        private readonly byte[] _silence;

        /// <summary>
        /// Constructor to load the Wave file from the stream into an AudioTrack
        /// </summary>
        public WaveFile(
            System.IO.Stream stream)
        {
            using (var reader = new BinaryReader(stream)) {
                var br = new BinaryReader(stream);
                var header = br.ReadInt32();
                if (header != ChunkIdentifierToInt32("RIFF")) {
                    throw new FormatException("Not a WAVE file - no RIFF header");
                }
                br.ReadUInt32();
                if (br.ReadInt32() != ChunkIdentifierToInt32("WAVE")) {
                    throw new FormatException("Not a WAVE file - no WAVE header");
                }

                // Loop around finding each chunk
                var dataChunkId = ChunkIdentifierToInt32("data");
                var formatChunkId = ChunkIdentifierToInt32("fmt ");
                int? format = null;
                var channels = 0;
                var sampleRate = 0;
                var bitsPerSample = 0;
                try {
                    while (format == null || _soundBuffer == null) {
                        var chunkIdentifier = br.ReadInt32();
                        var chunkLength = br.ReadUInt32();
                        if (chunkIdentifier == dataChunkId) {
                            _soundBuffer = new byte[chunkLength];
                            reader.Read(_soundBuffer, 0, (int)chunkLength);
                        } else if (chunkIdentifier == formatChunkId) {
                            if (chunkLength < 16) {
                                throw new InvalidDataException("Invalid WaveFormat Structure");
                            }
                            format = br.ReadUInt16();
                            channels = br.ReadInt16();
                            sampleRate = br.ReadInt32();
                            br.ReadInt32(); // averageBytesPerSecond
                            br.ReadInt16(); // blockAlign
                            bitsPerSample = br.ReadInt16();
                            if (chunkLength <= 16) continue;
                            var extraSize = br.ReadInt16();
                            if (extraSize != chunkLength - 18) {
                                extraSize = (short)(chunkLength - 18);
                            }
                            br.ReadBytes(extraSize);
                        } else {
                            br.ReadBytes((int)chunkLength);
                        }
                    }
                } catch {
                    if (format == null) {
                        throw new FormatException("Invalid WAV file - No fmt chunk found");
                    }
                    if (_soundBuffer == null) {
                        throw new FormatException("Invalid WAV file - No data chunk found");
                    }
                }
                if (format != 1) {
                    throw new FormatException("Invalid WAV file - we only support PCM");
                }

                // Create the audio track
                _audioTrack = new AudioTrack(
                    Android.Media.Stream.Music,
                    sampleRate,
                    channels == 1 ? ChannelOut.Mono : ChannelOut.Stereo,
                    bitsPerSample == 8 ? Encoding.Pcm8bit : Encoding.Pcm16bit,
                    _soundBuffer.Length,
                    AudioTrackMode.Stream);

                // We have to write silence to get it to always trigger for some reason
                _silence = new byte[_soundBuffer.Length];
            }
        }

        /// <summary>
        /// Called to dispose of the object
        /// </summary>
        public void Dispose()
        {
            _audioTrack?.Release();
            _audioTrack?.Dispose();
        }

        /// <summary>
        /// Chunk identifier to Int32
        /// </summary>
        /// <param name="s">Four character chunk identifier</param>
        /// <returns>Chunk identifier as int 32</returns>
        private static int ChunkIdentifierToInt32(
            string s)
        {
            return BitConverter.ToInt32(System.Text.Encoding.UTF8.GetBytes(s), 0);
        }

        /// <summary>
        /// Plays the wave file
        /// </summary>
        public void Play()
        {
            // Now play the audio track via a background task, but make sure only one can play at a time
            Task.Run(() => {
                lock (_audioTrack) {
                    _audioTrack.Play();
                    _audioTrack.Write(_soundBuffer, 0, _soundBuffer.Length);
                    _audioTrack.Write(_silence, 0, _silence.Length);
                }
            });
        }
    }
}

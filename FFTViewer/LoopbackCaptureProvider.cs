﻿using NAudio.Wave;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FFTViewer
{
    class LoopbackCaptureProvider : IAudioProvider
    {
        class Reader : IAudioReader
        {
            public Reader(LoopbackCaptureProvider provider, int len)
            {
                _Provider = provider;
                BufferLength = len;
                _Buffer = new CaptureBuffer<float>(len);

                _Provider._Capture.DataAvailable += Capture_DataAvailable;
            }

            private LoopbackCaptureProvider _Provider;
            private CaptureBuffer<float> _Buffer;

            public IAudioProvider Provider => _Provider;
            public int BufferLength { get; private set; }

            public void Dispose()
            {
                if (_Provider != null)
                {
                    _Provider._Capture.DataAvailable -= Capture_DataAvailable;
                    _Provider = null;
                }
            }

            private void Capture_DataAvailable(object sender, WaveInEventArgs e)
            {
                _Buffer.Write(e.Buffer, 0, e.BytesRecorded);
            }

            public void Read(float[] buffer)
            {
                if (_Provider == null) return;
                int channels = _Provider._Capture.WaveFormat.Channels;
                for (int i = 0; i < _Buffer._Data.Length; i += channels)
                {
                    buffer[i] = _Buffer._Data[i];
                }
            }
        }

        public LoopbackCaptureProvider()
        {
            _Capture = new WasapiLoopbackCapture();
            Rate = _Capture.WaveFormat.SampleRate;
            _Capture.StartRecording();
        }

        private WasapiLoopbackCapture _Capture;

        public event Action StateChanged;

        public int Rate { get; private set; }
        public int SourceCount => 1;
        public bool IsPlaying => true;

        public void Dispose()
        {
            if (_Capture != null)
            {
                _Capture.StopRecording();
                _Capture.Dispose();
                _Capture = null;
            }
        }

        public IAudioPlayControl GetPlayControl()
        {
            return new EmptyPlayControl();
        }

        public IAudioReader GetReader(int sourceIndex, int bufferLength)
        {
            return new Reader(this, bufferLength);
        }
    }
}

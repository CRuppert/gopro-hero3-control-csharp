using System.Collections.Generic;
using System.IO;

namespace Hero3Control
{
    public interface IVideoCameraControlService
    {
        void StartRecording();
        void StopRecording();
        void RecordForSeconds(int seconds);
        List<FileData> GetFileList();
        Stream GetLastFile();
    }
}
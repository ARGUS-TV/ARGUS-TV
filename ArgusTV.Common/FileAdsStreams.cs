using System;
using System.IO;
using System.Collections;
using System.Runtime.InteropServices;
using Microsoft.Win32.SafeHandles;

///<summary>
///Encapsulates access to alternative data streams of an NTFS file.
///Adapted from a C++ sample by Dino Esposito,
///http://msdn.microsoft.com/library/default.asp?url=/library/en-us/dnfiles/html/ntfs5.asp
///</summary>
namespace ArgusTV.Common
{
    /// <summary>
    /// Wraps the API functions, structures and constants.
    /// </summary>
    internal class Kernel32
    {
        public const char STREAM_SEP = ':';
        public const int INVALID_HANDLE_VALUE = -1;
        public const int MAX_PATH = 256;

        [Flags()]
        public enum FileFlags : uint
        {
            WriteThrough = 0x80000000,
            Overlapped = 0x40000000,
            NoBuffering = 0x20000000,
            RandomAccess = 0x10000000,
            SequentialScan = 0x8000000,
            DeleteOnClose = 0x4000000,
            BackupSemantics = 0x2000000,
            PosixSemantics = 0x1000000,
            OpenReparsePoint = 0x200000,
            OpenNoRecall = 0x100000
        }

        [Flags()]
        public enum FileAccessAPI : uint
        {
            GENERIC_READ = 0x80000000,
            GENERIC_WRITE = 0x40000000
        }
        /// <summary>
        /// Provides a mapping between a System.IO.FileAccess value and a FileAccessAPI value.
        /// </summary>
        /// <param name="Access">The <see cref="System.IO.FileAccess"/> value to map.</param>
        /// <returns>The <see cref="FileAccessAPI"/> value.</returns>
        public static FileAccessAPI Access2API(FileAccess Access)
        {
            FileAccessAPI lRet = 0;
            if ((Access & FileAccess.Read) == FileAccess.Read) lRet |= FileAccessAPI.GENERIC_READ;
            if ((Access & FileAccess.Write) == FileAccess.Write) lRet |= FileAccessAPI.GENERIC_WRITE;
            return lRet;
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct LARGE_INTEGER
        {
            public int Low;
            public int High;

            public long ToInt64()
            {
                return (long)High * 4294967296 + (long)Low;
            }
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct WIN32_STREAM_ID
        {
            public int dwStreamID;
            public int dwStreamAttributes;
            public LARGE_INTEGER Size;
            public int dwStreamNameSize;
        }

        [DllImport("kernel32")]
        public static extern IntPtr CreateFile(string name, [MarshalAs(UnmanagedType.U4)] FileAccessAPI access, [MarshalAs(UnmanagedType.U4)] FileShare share,
            IntPtr security, [MarshalAs(UnmanagedType.U4)] FileMode creation, [MarshalAs(UnmanagedType.U4)] FileFlags flags, IntPtr template);
        [DllImport("kernel32")]
        public static extern bool DeleteFile(string name);
        [DllImport("kernel32")]
        public static extern bool CloseHandle(IntPtr hObject);

        [DllImport("kernel32")]
        public static extern bool BackupRead(IntPtr hFile, IntPtr lpBuffer, uint nNumberOfBytesToRead, out uint lpNumberOfBytesRead, bool bAbort, bool bProcessSecurity, ref IntPtr lpContext);
        [DllImport("kernel32")]
        public static extern bool BackupRead(IntPtr hFile, ref WIN32_STREAM_ID pBuffer, uint nNumberOfBytesToRead, out uint lpNumberOfBytesRead, bool bAbort, bool bProcessSecurity, ref IntPtr lpContext);
        [DllImport("kernel32.dll")]
        public static extern bool BackupSeek(IntPtr hFile, uint dwLowBytesToSeek, uint dwHighBytesToSeek, out uint lpdwLowBytesSeeked, out uint lpdwHighBytesSeeked, [In] ref IntPtr lpContext);
    }

    /// <summary>
    /// Encapsulates a single alternative data stream for a file.
    /// </summary>
    public class StreamInfo
    {
        private FileAdsStreams _parent;
        private string _name;
        private long _size;

        internal StreamInfo(FileAdsStreams parent, string name, long size)
        {
            _parent = parent;
            _name = name;
            _size = size;
        }

        /// <summary>
        /// The name of the stream.
        /// </summary>
        public string Name
        {
            get { return _name; }
        }
        /// <summary>
        /// The size (in bytes) of the stream.
        /// </summary>
        public long Size
        {
            get { return _size; }
        }

        public override string ToString()
        {
            return String.Format("{1}{0}{2}{0}$DATA", Kernel32.STREAM_SEP, _parent.FileName, _name);
        }

        public override bool Equals(Object o)
        {
            if (o is StreamInfo)
            {
                StreamInfo f = (StreamInfo)o;
                return (f._name.Equals(_name) && f._parent.Equals(_parent));
            }
            else if (o is string)
            {
                return ((string)o).Equals(ToString());
            }
            else
                return base.Equals(o);
        }

        public override int GetHashCode()
        {
            return ToString().GetHashCode();
        }

        #region Open

        /// <summary>
        /// Opens or creates the stream in read-write mode, with no sharing.
        /// </summary>
        /// <returns>A <see cref="System.IO.FileStream"/> wrapper for the stream.</returns>
        public FileStream Open()
        {
            return Open(FileMode.OpenOrCreate, FileAccess.ReadWrite, FileShare.None);
        }

        /// <summary>
        /// Opens or creates the stream in read-write mode with no sharing.
        /// </summary>
        /// <param name="mode">The <see cref="System.IO.FileMode"/> action for the stream.</param>
        /// <returns>A <see cref="System.IO.FileStream"/> wrapper for the stream.</returns>
        public FileStream Open(FileMode mode)
        {
            return Open(mode, FileAccess.ReadWrite, FileShare.None);
        }

        /// <summary>
        /// Opens or creates the stream with no sharing.
        /// </summary>
        /// <param name="mode">The <see cref="System.IO.FileMode"/> action for the stream.</param>
        /// <param name="access">The <see cref="System.IO.FileAccess"/> level for the stream.</param>
        /// <returns>A <see cref="System.IO.FileStream"/> wrapper for the stream.</returns>
        public FileStream Open(FileMode mode, FileAccess access)
        {
            return Open(mode, access, FileShare.None);
        }

        /// <summary>
        /// Opens or creates the stream.
        /// </summary>
        /// <param name="mode">The <see cref="System.IO.FileMode"/> action for the stream.</param>
        /// <param name="access">The <see cref="System.IO.FileAccess"/> level for the stream.</param>
        /// <param name="share">The <see cref="System.IO.FileShare"/> level for the stream.</param>
        /// <returns>A <see cref="System.IO.FileStream"/> wrapper for the stream.</returns>
        public FileStream Open(FileMode mode, FileAccess access, FileShare share)
        {
            try
            {
                IntPtr hFile = Kernel32.CreateFile(ToString(), Kernel32.Access2API(access), share, IntPtr.Zero, mode, 0, IntPtr.Zero);
                if (hFile != new IntPtr(-1))
                {
                    return new FileStream(new SafeFileHandle(hFile, true), access);
                }
            }
            catch
            {
            }
            return null;
        }

        #endregion

        #region Delete

        /// <summary>
        /// Deletes the stream from the file.
        /// </summary>
        /// <returns>A <see cref="System.Boolean"/> value: true if the stream was deleted, false if there was an error.</returns>
        public bool Delete()
        {
            return Kernel32.DeleteFile(ToString());
        }

        #endregion
    }

    /// <summary>
    /// Encapsulates the collection of alternative data streams for a file.
    /// A collection of <see cref="StreamInfo"/> objects.
    /// </summary>
    public class FileAdsStreams : CollectionBase
    {
        private FileInfo _file;

        #region Constructors

        public FileAdsStreams(string File)
        {
            _file = new FileInfo(File);
            InitStreams();
        }

        public FileAdsStreams(FileInfo file)
        {
            _file = file;
            InitStreams();
        }

        /// <summary>
        /// Reads the streams from the file.
        /// </summary>
        private void InitStreams()
        {
            //Open the file with backup semantics
            IntPtr hFile = Kernel32.CreateFile(_file.FullName, Kernel32.FileAccessAPI.GENERIC_READ, FileShare.Read, IntPtr.Zero, FileMode.Open, Kernel32.FileFlags.BackupSemantics, IntPtr.Zero);
            if (hFile.ToInt32() == Kernel32.INVALID_HANDLE_VALUE) return;

            try
            {
                Kernel32.WIN32_STREAM_ID sid = new Kernel32.WIN32_STREAM_ID();
                uint dwStreamHeaderSize = (uint)Marshal.SizeOf(sid);
                IntPtr context = IntPtr.Zero;
                bool @continue = true;
                while (@continue)
                {
                    //Read the next stream header
                    uint lRead = 0;
                    @continue = Kernel32.BackupRead(hFile, ref sid, dwStreamHeaderSize, out lRead, false, false, ref context);
                    if (@continue && lRead == dwStreamHeaderSize)
                    {
                        if (sid.dwStreamNameSize > 0)
                        {
                            //Read the stream name
                            lRead = 0;
                            IntPtr pName = Marshal.AllocHGlobal(sid.dwStreamNameSize);
                            try
                            {
                                @continue = Kernel32.BackupRead(hFile, pName, (uint)sid.dwStreamNameSize, out lRead, false, false, ref context);
                                char[] bName = new char[sid.dwStreamNameSize];
                                Marshal.Copy(pName, bName, 0, sid.dwStreamNameSize);

                                //Name is of the format ":NAME:$DATA\0"
                                string sName = new string(bName);
                                int i = sName.IndexOf(Kernel32.STREAM_SEP, 1);
                                if (i > -1) sName = sName.Substring(1, i - 1);
                                else
                                {
                                    //This should never happen. 
                                    //Truncate the name at the first null char.
                                    i = sName.IndexOf('\0');
                                    if (i > -1) sName = sName.Substring(1, i - 1);
                                }

                                //Add the stream to the collection
                                base.List.Add(new StreamInfo(this, sName, sid.Size.ToInt64()));
                            }
                            finally
                            {
                                Marshal.FreeHGlobal(pName);
                            }
                        }

                        //Skip the stream contents
                        uint l; uint h;
                        @continue = Kernel32.BackupSeek(hFile, (uint)sid.Size.Low, (uint)sid.Size.High, out l, out h, ref context);
                    }
                    else break;
                }
            }
            finally
            {
                Kernel32.CloseHandle(hFile);
            }
        }
        #endregion

        #region File
        /// <summary>
        /// Returns the <see cref="System.IO.FileInfo"/> object for the wrapped file. 
        /// </summary>
        public FileInfo FileInfo
        {
            get { return _file; }
        }

        /// <summary>
        /// Returns the full path to the wrapped file.
        /// </summary>
        public string FileName
        {
            get { return _file.FullName; }
        }

        /// <summary>
        /// Returns the size of the main data stream, in bytes.
        /// </summary>
        public long FileSize
        {
            get { return _file.Length; }
        }

        /// <summary>
        /// Returns the size of all streams for the file, in bytes.
        /// </summary>
        public long Size
        {
            get
            {
                long size = this.FileSize;
                foreach (StreamInfo s in this) size += s.Size;
                return size;
            }
        }
        #endregion

        #region Open
        /// <summary>
        /// Opens or creates the default file stream.
        /// </summary>
        /// <returns><see cref="System.IO.FileStream"/></returns>
        public FileStream Open()
        {
            return new FileStream(_file.FullName, FileMode.OpenOrCreate);
        }

        /// <summary>
        /// Opens or creates the default file stream.
        /// </summary>
        /// <param name="mode">The <see cref="System.IO.FileMode"/> for the stream.</param>
        /// <returns><see cref="System.IO.FileStream"/></returns>
        public FileStream Open(FileMode mode)
        {
            return new FileStream(_file.FullName, mode);
        }

        /// <summary>
        /// Opens or creates the default file stream.
        /// </summary>
        /// <param name="mode">The <see cref="System.IO.FileMode"/> for the stream.</param>
        /// <param name="access">The <see cref="System.IO.FileAccess"/> for the stream.</param>
        /// <returns><see cref="System.IO.FileStream"/></returns>
        public FileStream Open(FileMode mode, FileAccess access)
        {
            return new FileStream(_file.FullName, mode, access);
        }

        /// <summary>
        /// Opens or creates the default file stream.
        /// </summary>
        /// <param name="mode">The <see cref="System.IO.FileMode"/> for the stream.</param>
        /// <param name="access">The <see cref="System.IO.FileAccess"/> for the stream.</param>
        /// <param name="share">The <see cref="System.IO.FileShare"/> for the stream.</param>
        /// <returns><see cref="System.IO.FileStream"/></returns>
        public FileStream Open(FileMode mode, FileAccess access, FileShare share)
        {
            return new FileStream(_file.FullName, mode, access, share);
        }
        #endregion

        #region Delete
        /// <summary>
        /// Deletes the file, and all alternative streams.
        /// </summary>
        public void Delete()
        {
            for (int i = base.List.Count; i > 0; i--)
            {
                base.List.RemoveAt(i);
            }
            _file.Delete();
        }
        #endregion

        #region Collection operations
        /// <summary>
        /// Add an alternative data stream to this file.
        /// </summary>
        /// <param name="name">The name for the stream.</param>
        /// <returns>The index of the new item.</returns>
        public int Add(string name)
        {
            StreamInfo FSI = new StreamInfo(this, name, 0);
            int i = base.List.IndexOf(FSI);
            if (i == -1) i = base.List.Add(FSI);
            return i;
        }
        /// <summary>
        /// Removes the alternative data stream with the specified name.
        /// </summary>
        /// <param name="name">The name of the string to remove.</param>
        public void Remove(string name)
        {
            StreamInfo FSI = new StreamInfo(this, name, 0);
            int i = base.List.IndexOf(FSI);
            if (i > -1) base.List.RemoveAt(i);
        }

        /// <summary>
        /// Returns the index of the specified <see cref="StreamInfo"/> object in the collection.
        /// </summary>
        /// <param name="fsi">The object to find.</param>
        /// <returns>The index of the object, or -1.</returns>
        public int IndexOf(StreamInfo fsi)
        {
            return base.List.IndexOf(fsi);
        }
        /// <summary>
        /// Returns the index of the <see cref="StreamInfo"/> object with the specified name in the collection.
        /// </summary>
        /// <param name="name">The name of the stream to find.</param>
        /// <returns>The index of the stream, or -1.</returns>
        public int IndexOf(string name)
        {
            return base.List.IndexOf(new StreamInfo(this, name, 0));
        }

        public StreamInfo this[int index]
        {
            get { return (StreamInfo)base.List[index]; }
        }

        public StreamInfo this[string name]
        {
            get
            {
                int i = IndexOf(name);
                if (i == -1) return null;
                else return (StreamInfo)base.List[i];
            }
        }
        #endregion

        #region Overrides
        /// <summary>
        /// Throws an exception if you try to add anything other than a StreamInfo object to the collection.
        /// </summary>
        protected override void OnInsert(int index, object value)
        {
            if (!(value is StreamInfo)) throw new InvalidCastException();
        }
        /// <summary>
        /// Throws an exception if you try to add anything other than a StreamInfo object to the collection.
        /// </summary>
        protected override void OnSet(int index, object oldValue, object newValue)
        {
            if (!(newValue is StreamInfo)) throw new InvalidCastException();
        }

        /// <summary>
        /// Deletes the stream from the file when you remove it from the list.
        /// </summary>
        protected override void OnRemoveComplete(int index, object value)
        {
            try
            {
                StreamInfo FSI = (StreamInfo)value;
                if (FSI != null) FSI.Delete();
            }
            catch { }
        }

        public new StreamEnumerator GetEnumerator()
        {
            return new StreamEnumerator(this);
        }
        #endregion

        #region StreamEnumerator
        public class StreamEnumerator : object, IEnumerator
        {
            private IEnumerator baseEnumerator;

            public StreamEnumerator(FileAdsStreams mappings)
            {
                this.baseEnumerator = ((IEnumerable)(mappings)).GetEnumerator();
            }

            public StreamInfo Current
            {
                get
                {
                    return ((StreamInfo)(baseEnumerator.Current));
                }
            }

            object IEnumerator.Current
            {
                get
                {
                    return baseEnumerator.Current;
                }
            }

            public bool MoveNext()
            {
                return baseEnumerator.MoveNext();
            }

            bool IEnumerator.MoveNext()
            {
                return baseEnumerator.MoveNext();
            }

            public void Reset()
            {
                baseEnumerator.Reset();
            }

            void IEnumerator.Reset()
            {
                baseEnumerator.Reset();
            }
        }
        #endregion
    }
}

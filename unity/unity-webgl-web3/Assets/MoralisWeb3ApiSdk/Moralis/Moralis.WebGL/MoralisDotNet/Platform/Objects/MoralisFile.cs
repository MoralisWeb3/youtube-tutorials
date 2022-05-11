using System;
using System.IO;
using Moralis.WebGL.Platform.Utilities;

namespace Moralis.WebGL.Platform.Objects
{
    public class MoralisFile
    {
        internal MoralisFileState State { get; set; }

        internal Stream DataStream { get; }

        internal TaskQueue TaskQueue { get; } = new TaskQueue { };

        #region Constructor

        internal MoralisFile(string name, Uri uri, string mimeType = null) => State = new MoralisFileState
        {
            name = name,
            url = uri,
            mediatype = mimeType
        };

        /// <summary>
        /// Creates a new file from a byte array and a name.
        /// </summary>
        /// <param name="name">The file's name, ideally with an extension. The file name
        /// must begin with an alphanumeric character, and consist of alphanumeric
        /// characters, periods, spaces, underscores, or dashes.</param>
        /// <param name="data">The file's data.</param>
        /// <param name="mimeType">To specify the content-type used when uploading the
        /// file, provide this parameter.</param>
        public MoralisFile(string name, byte[] data, string mimeType = null) : this(name, new MemoryStream(data), mimeType) { }

        /// <summary>
        /// Creates a new file from a stream and a name.
        /// </summary>
        /// <param name="name">The file's name, ideally with an extension. The file name
        /// must begin with an alphanumeric character, and consist of alphanumeric
        /// characters, periods, spaces, underscores, or dashes.</param>
        /// <param name="data">The file's data.</param>
        /// <param name="mimeType">To specify the content-type used when uploading the
        /// file, provide this parameter.</param>
        public MoralisFile(string name, Stream data, string mimeType = null)
        {
            State = new MoralisFileState
            {
                name = name,
                mediatype = mimeType
            };

            DataStream = data;
        }

        #endregion

        #region Properties

        /// <summary>
        /// Gets whether the file still needs to be saved.
        /// </summary>
        public bool IsDirty => State.url == null;

        /// <summary>
        /// Gets the name of the file. Before save is called, this is the filename given by
        /// the user. After save is called, that name gets prefixed with a unique identifier.
        /// </summary>
        //[JsonProperty("name")]
        public string name => State.name;

        /// <summary>
        /// Gets the MIME type of the file. This is either passed in to the constructor or
        /// inferred from the file extension. "unknown/unknown" will be used if neither is
        /// available.
        /// </summary>
        public string MimeType => State.mediatype;

        /// <summary>
        /// Gets the url of the file. It is only available after you save the file or after
        /// you get the file from a <see cref="ParseObject"/>.
        /// </summary>
        //[JsonProperty("url")]
        public Uri url => State.SecureLocation;

        #endregion
    }
}

using System;
using System.IO;

namespace UltraForce.Library.NetStandard.IO
{
  /// <summary>
  /// <see cref="UFLazyStream"/> is a wrapper for another stream. The wrapped stream gets only instantiated if any of
  /// the stream methods or properties are accessed. For example this class can be used in combination with a
  /// file creation stream, creating only the file if the stream instance is accessed.
  /// </summary>
  public class UFLazyStream : Stream
  {
    #region private variables

    /// <summary>
    /// The stream factory
    /// </summary>
    private readonly Func<Stream> m_factory;

    /// <summary>
    /// Wrapped stream
    /// </summary>
    private Stream? m_stream;

    #endregion

    #region constructors & destructors

    /// <summary>
    /// Constructs an instance of <see cref="UFLazyStream"/>
    /// </summary>
    /// <param name="aFactory">A factory that creates the wrapped stream</param>
    public UFLazyStream(Func<Stream> aFactory)
    {
      this.m_factory = aFactory;
    }

    /// <inheritdoc />
    protected override void Dispose(bool disposing)
    {
      this.m_stream?.Dispose();
      base.Dispose(disposing);
    }

    #endregion

    #region Stream

    /// <inheritdoc />
    public override void Flush()
    {
      this.Get().Flush();
    }

    /// <inheritdoc />
    public override int Read(byte[] buffer, int offset, int count)
    {
      return this.Get().Read(buffer, offset, count);
    }

    /// <inheritdoc />
    public override long Seek(long offset, SeekOrigin origin)
    {
      return this.Get().Seek(offset, origin);
    }

    /// <inheritdoc />
    public override void SetLength(long value)
    {
      this.Get().SetLength(value);
    }

    /// <inheritdoc />
    public override void Write(byte[] buffer, int offset, int count)
    {
      this.Get().Write(buffer, offset, count);
    }

    /// <inheritdoc />
    public override bool CanRead => this.Get().CanRead;

    /// <inheritdoc />
    public override bool CanSeek => this.Get().CanSeek;

    /// <inheritdoc />
    public override bool CanWrite => this.Get().CanWrite;

    /// <inheritdoc />
    public override long Length => this.Get().Length;

    /// <inheritdoc />
    public override long Position {
      get => this.Get().Position;
      set => this.Get().Position = value;
    }

    #endregion

    #region private methods

    /// <summary>
    /// Returns the stream, the first call will call the factory to create the stream.
    /// </summary>
    /// <returns>Stream</returns>
    private Stream Get()
    {
      return this.m_stream ?? (this.m_stream = this.m_factory());
    }

    #endregion
  }
}
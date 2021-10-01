# Changelog


## Version 2.2.0
### Features
- Support for Little and Big Endianness, this can be configured on ReBuffer::SetEndianness and will default to the endianness set in BitConverter::IsLittleEndian.
    this change shouldn't affect current API

## Version 2.1.0
### Features
- Added ReBuffer::ShrinkFit to resize internal array to fit used space of buffer
- Added Unity packer for Matrix4x4, Bounds, BoundsInt, Rect, RectInt, Rigidbody and Rigidbody2D
- Added ReBuffer::GetRef to get a reference to an unmanaged value type  
    this reference can then be change to modify the value in the buffer

### Changes
- ReBuffer::Copy using WriteCursor instead of Length
- ReBuffer::Clone using write cursor instead of Length for new buffers size

### Fixes
- ReBuffer::SetWriteCursor/SetReadCursor not checking for lower bounds
- ReBuffer::CanWriteBytes not checking correct bounds
- more test cases for ReBuffer
- clarified offset parameter on ReBuffer::Peek as byteOffset
- Fixed wrapper for UnityEngine::Rect

## Version 2.0.0

### Features
- Peek an unmanaged value on the buffer
- Added CanRead and CanReadBytes methods to Buffer
- Added "expand" parameter to Buffer constructor, allowing it to grow in size to fit data.  
    used automatically with `RePacking.Pack<T>(ref T value)`.
- Added runtime SizeOf calculation for tagged and unmanaged objects  
    used with `RePacking.SizeOf<T>(ref T value)`
    any custom wrappers needs to override `RePackerWrapper.SizeOf` unless they're unmanaged/directly copyable
- Copy one buffer into another, as long as the destination can fit it
- Support for ValueTuple<T1> and ValueTuple with TRest

### Changes
- Buffer is now a class and renamed to ReBuffer, replacing BoxedBuffer entirely
- Moved packing/unpacking utils from Buffer into extension class
- renamed Buffer::CanFit to CanWrite
- renamed Buffer::CanFitBytes to CanWriteBytes
- renamed Buffer::MemoryCopyFromUnsafe to Buffer::PackArray
- renamed Buffer::MemoryCopyToUnsafe to Buffer::UnpackArray
- renamed Pop/Push on Buffer extensions to Pack/Unpack
- bunch of other smaller changes to names/structure

### Fixes
- null ref in Buffer::MemoryCopyFromUnsafe
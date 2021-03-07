# Changelog

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
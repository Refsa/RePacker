# Changelog

## Version 2.0.0

### Features
- Copy one buffer into another, as long as the destination can fit it
- Peek an unmanaged value on the buffer
- Added CanRead and CanReadBytes methods to Buffer
- Added "expand" parameter to Buffer constructor, allowing it to grow in size to fit data.
used in automatically with `RePacking.Pack<T>(ref T value)`.

### Changes
- Buffer is now a class, replacing BoxedBuffer entirely
- Moved packing/unpacking utils from Buffer into extension class
- renamed Buffer::CanFit to CanWrite
- renamed Buffer::CanFitBytes to CanWriteBytes
- renamed Buffer::MemoryCopyFromUnsafe to Buffer::PackArray
- renamed Buffer::MemoryCopyToUnsafe to Buffer::UnpackArray
- renamed Pop/Push on Buffer extensions to Pack/Unpack
- bunch of other smaller changes to names/structure

### Fixes
- null ref in Buffer::MemoryCopyFromUnsafe
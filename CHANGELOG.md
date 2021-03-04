# Changelog

## Version 2.0.0

### Features
- Copy one buffer into another, as long as the destination can fit it
- Peek an unmanaged value on the buffer
- Added CanRead and CanReadBytes

### Changes
- Buffer is now a class, replacing BoxedBuffer entirely
- Moved packing/unpacking utils from Buffer into extension class
- renamed Buffer::CanFit to CanWrite
- renamed Buffer::CanFitBytes to CanWriteBytes
- renamed Buffer::MemoryCopyFromUnsafe to Buffer::PackArray
- renamed Buffer::MemoryCopyToUnsafe to Buffer::UnpackArray

### Fixes
- null ref in Buffer::MemoryCopyFromUnsafe
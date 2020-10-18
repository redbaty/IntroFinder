# :film_projector: IntroFinder
This project aims to provide a fast and reliable way to auto-detect TV shows intros. Currently at an alpha state.

# Dependencies
* [:movie_camera: FFmpeg](https://github.com/FFmpeg/FFmpeg) - for extracting frames from the video files.
* [:framed_picture: ImageSharp](https://github.com/SixLabors/ImageSharp) and [ImageHash](https://github.com/coenm/ImageHash) - for calculating average image hash. 
* [:desktop_computer: CliWrap](https://github.com/Tyrrrz/CliWrap) - for calling FFmpeg and piping its outputs.

# Limitations
* Currently only `.mkv` files are found on the specified directory, and will be tracked on [this issue](https://github.com/redbaty/IntroFinder/issues/1).
* Currently none of the options can be changed, so [these defaults](https://github.com/redbaty/IntroFinder/blob/master/IntroFinder.Core/Models/FrameFinderOptions.cs will be used to determined a 'valid intro'.
* If there's a recap that matches the frames of the previous episode it is possible that it will be detected as an intro part.

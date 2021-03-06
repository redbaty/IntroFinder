# :film_projector: IntroFinder
This project aims to provide a fast and reliable way to auto-detect TV shows intros. Currently at an alpha state.

# Dependencies
* [:movie_camera: FFmpeg](https://github.com/FFmpeg/FFmpeg) and [FFmpeg.FrameExtractor](https://github.com/redbaty/FFmpeg.FrameExtractor) - for extracting frames from the video files.
* [:framed_picture: ImageSharp](https://github.com/SixLabors/ImageSharp) and [ImageHash](https://github.com/coenm/ImageHash) - for calculating average image hash. 
* [:desktop_computer: CliWrap](https://github.com/Tyrrrz/CliWrap) - for calling FFmpeg and piping its outputs.
* [:computer: CommandLineParser](https://github.com/commandlineparser/commandline) - for parsing console arguments.
* [:page_with_curl: Microsoft.Extensions.Logging.Abstractions](https://docs.microsoft.com/en-us/aspnet/core/fundamentals/logging/?view=aspnetcore-3.1) and [Serilog](https://serilog.net/) - for logging.

# Limitations
* If there's a recap that matches the frames of the previous episode it is possible that it will be detected as an intro part.

# Interactive Registration Methods for Augmented Reality in Robotics

## Overview 
This codes provides an implementation of the research paper:

```
"Interactive Registration Methods for Augmented Reality in Robotics: A Comparative Evaluation"
Tonia Mielke, Fabian Joeres, Danny Schott, Christian Hansen 
``` 

It contains the implementation of three registration methods for the registration of AR content in a robotic workspace
- Board registration: point-based registration utilizing a 3D-printed board to physically define registration points 
- Mid-Air registration: point-based registration using Vuforia image marker attached to the robot end-effector to aquire registration points mid-air 
- Manual: registration by iteratively manually adjusting the position and orientation of a life size model

## License
``` 
MIT License

Copyright (c) 2023 Tonia Mielke

Permission is hereby granted, free of charge, to any person obtaining a copy
of this software and associated documentation files (the "Software"), to deal
in the Software without restriction, including without limitation the rights
to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
copies of the Software, and to permit persons to whom the Software is
furnished to do so, subject to the following conditions:

The above copyright notice and this permission notice shall be included in all
copies or substantial portions of the Software.

THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
SOFTWARE.
``` 
## Dependencies 
- [MRTK](https://github.com/microsoft/MixedRealityToolkit-Unity)
- [Vuforia](https://library.vuforia.com/mat)
- [Mathnet Numerics](https://github.com/mathnet/mathnet-numerics)

## Citing 

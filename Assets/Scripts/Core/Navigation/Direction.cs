// ---------------------------------------------------------------------------------------------
//  Copyright (c) 2021-2023, Jiaqi Liu. All rights reserved.
//  See LICENSE file in the project root for license information.
// ---------------------------------------------------------------------------------------------

namespace Core.Navigation
{
    /*
     *  Use integer to represent 8 directions
     *
     *  5   4   3
     *      |
     *  6 -- -- 2
     *      |
     *  7   0   1
     *
     */

    public enum Direction
    {
        South       =    0,
        SouthEast   =    1,
        East        =    2,
        NorthEast   =    3,
        North       =    4,
        NorthWest   =    5,
        West        =    6,
        SouthWest   =    7,
    }
}
using System.Collections;
using System.Collections.Generic;

public static class UFListExtensions
{
    public static void Move(this IList list, int iIndexToMove, bool up = true)
    {
        if (up)
        {
            var move = iIndexToMove - 1;
            if (move < 0 || move >= list.Count) return;
            var old = list[move];
            list[move] = list[iIndexToMove];
            list[iIndexToMove] = old;
        }
        else
        {

            var move = iIndexToMove + 1;
            if (move < 0 || move >= list.Count) return;
            var old = list[move];
            list[move] = list[iIndexToMove];
            list[iIndexToMove] = old;
        }
    }
}
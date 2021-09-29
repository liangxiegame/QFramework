////////////////////////////////////////////////////////////////////////////////

using UnityEngine;

namespace MG.MDV
{
    public interface IActions
    {
        Texture FetchImage( string url );
        void    SelectPage( string url );
    }
}


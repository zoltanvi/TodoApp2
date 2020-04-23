using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TodoApp2.Animations
{
    /// <summary>
    /// Styles of page animations for appearing/disappearing
    /// </summary>
    public enum PageAnimation
    {
        /// <summary>
        /// No animation takes place
        /// </summary>
        None = 0,

        /// <summary>
        /// The page fades in
        /// </summary>
        FadeIn = 1,
        
        /// <summary>
        /// The page fades out
        /// </summary>
        FadeOut = 2
    }
}

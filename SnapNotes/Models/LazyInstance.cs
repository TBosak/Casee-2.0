using Microsoft.Extensions.DependencyInjection;
using System;

namespace SnapNotes.Models
{
    public class LazyInstance<T> : Lazy<T>
    {
        public LazyInstance(IServiceProvider serviceProvider) : base(() => serviceProvider.GetRequiredService<T>())
        {

        }
    }
}

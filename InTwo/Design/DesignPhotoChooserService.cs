using System;
using System.Threading.Tasks;
using Cimbalino.Phone.Toolkit.Services;
using Microsoft.Phone.Tasks;

namespace InTwo.Design
{
    public class DesignPhotoChooserService : IPhotoChooserService
    {
        public void Show(Action<PhotoResult> resultAction)
        {
            
        }

        public void Show(bool showCamera, Action<PhotoResult> resultAction)
        {
        }

        public Task<PhotoResult> ShowAsync()
        {
            return new Task<PhotoResult>(null);
        }

        public Task<PhotoResult> ShowAsync(bool showCamera)
        {
            return new Task<PhotoResult>(null);
        }
    }
}
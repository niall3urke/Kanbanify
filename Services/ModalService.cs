using Microsoft.AspNetCore.Components;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Kanbanify.Services
{
    public class ModalService
    {

        // Events 

        public Action<RenderFragment, ModalParameters> ShowModal;

        public Action<ModalResult> OnClose;

        internal Action CloseModal;

        // Methods

        public void Show<T>(ModalParameters parameters) where T : ComponentBase
        {
            var content = new RenderFragment(x =>
            {
                x.OpenComponent(1, typeof(T));
                x.CloseComponent();
            });

            ShowModal?.Invoke(content, parameters);
        }

        public void Show<T>() where T : ComponentBase
        {
            var content = new RenderFragment(x =>
            {
                x.OpenComponent(1, typeof(T));
                x.CloseComponent();
            });

            ShowModal?.Invoke(content, new ModalParameters());
        }

        public void Close(ModalResult result)
        {
            OnClose?.Invoke(result);
            CloseModal?.Invoke();
        }


    }
}

using Kanbanify.Services;
using Microsoft.AspNetCore.Components;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Kanbanify.Shared
{
    public partial class Modal : IDisposable
    {

        // Injects

        [Inject] ModalService Service { get; set; }

        // Properties

        protected ModalParameters Parameters { get; set; }

        protected RenderFragment Content { get; set; }

        protected bool Visible { get; set; }

        // Events

        protected override void OnInitialized()
        {
            base.OnInitialized();

            Service.ShowModal += ShowModal;
            Service.CloseModal += CloseModal;
        }

        // Handlers

        void ShowModal(RenderFragment content, ModalParameters parameters)
        {
            Parameters = parameters;
            Content = content;
            Visible = true;

            StateHasChanged();
        }

        void CloseModal()
        {
            Visible = false;
            Content = null;

            StateHasChanged();
        }

        void OnClickBackground()
        {
            Service.Close(ModalResult.Cancel());
        }

        // Methods - interfaces

        public void Dispose()
        {
            Service.ShowModal -= ShowModal;
            Service.CloseModal -= CloseModal;
        }
    }
}

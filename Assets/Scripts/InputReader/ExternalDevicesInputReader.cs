using Core.Services.Updater;
using System;
using System.Linq;
using UnityEngine;
using UnityEngine.EventSystems;

namespace InputReader
{
    public class ExternalDevicesInputReader : IEntityInputSource, IDisposable
    {
        public float HorizontalDirection => Input.GetAxisRaw("Horizontal");
        public float VerticalDirection => Input.GetAxisRaw("Vertical");
        public bool Jump { get; private set; }
        public bool Attack { get; private set; }

        public ExternalDevicesInputReader()
        {
            ProjectUpdater.Instance.UpdateCalled += OnUpdate;
        }
        public void ResetOneTimeActions()
        {
            Jump = false;
            Attack = false;
        }
        public void Dispose() => ProjectUpdater.Instance.UpdateCalled -= OnUpdate;
        private void OnUpdate()
        {
            if (Input.GetButtonDown("Jump"))
                Jump = true;

            if (!IsPointerOverUI() && Input.GetButtonDown("Fire1") && !Input.touches.Any())
                Attack = true;
        }
        private bool IsPointerOverUI() => EventSystem.current.IsPointerOverGameObject();
    }
}

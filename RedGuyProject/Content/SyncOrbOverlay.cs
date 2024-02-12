using UnityEngine.Networking;
using R2API.Networking;
using R2API.Networking.Interfaces;
using UnityEngine;
using RoR2;

namespace RedGuyMod.Content
{
    internal class SyncOrbOverlay : INetMessage
    {
        private NetworkInstanceId netId;
        private GameObject target;

        public SyncOrbOverlay()
        {
        }

        public SyncOrbOverlay(NetworkInstanceId netId, GameObject target)
        {
            this.netId = netId;
            this.target = target;
        }

        public void Deserialize(NetworkReader reader)
        {
            this.netId = reader.ReadNetworkId();
            this.target = reader.ReadGameObject();
        }

        public void OnReceived()
        {
            GameObject bodyObject = Util.FindNetworkObject(this.netId);
            if (!bodyObject)
            {
                Chat.AddMessage("Fuck");
                return;
            }

            RedGuyMod.Content.Components.RedGuyController penis = bodyObject.GetComponent<RedGuyMod.Content.Components.RedGuyController>();
            if (penis)
            {
                Transform modelTransform = penis.characterBody.modelLocator.modelTransform;
                if (modelTransform && penis.skinDef)
                {
                    TemporaryOverlay temporaryOverlay = modelTransform.gameObject.AddComponent<TemporaryOverlay>();
                    temporaryOverlay.duration = 1f;
                    temporaryOverlay.destroyComponentOnEnd = true;
                    temporaryOverlay.originalMaterial = penis.skinDef.bloodOrbOverlayMaterial;
                    temporaryOverlay.inspectorCharacterModel = modelTransform.GetComponent<CharacterModel>();
                    temporaryOverlay.alphaCurve = AnimationCurve.EaseInOut(0f, 1f, 1f, 0f);
                    temporaryOverlay.animateShaderAlpha = true;
                }

                Util.PlaySound(penis.skinDef.consumeSoundString, this.target.gameObject);
            }
        }

        public void Serialize(NetworkWriter writer)
        {
            writer.Write(this.netId);
            writer.Write(this.target);
        }
    }
}
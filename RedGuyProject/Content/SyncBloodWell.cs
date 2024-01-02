using UnityEngine.Networking;
using R2API.Networking;
using R2API.Networking.Interfaces;
using UnityEngine;
using RoR2;

namespace RedGuyMod.Content
{
    internal class SyncBloodWell : INetMessage
    {
        private NetworkInstanceId netId;
        private ulong fill;

        public SyncBloodWell()
        {
        }

        public SyncBloodWell(NetworkInstanceId netId, ulong augh)
        {
            this.netId = netId;
            this.fill = augh;
        }

        public void Deserialize(NetworkReader reader)
        {
            this.netId = reader.ReadNetworkId();
            this.fill = reader.ReadUInt64();
        }

        public void OnReceived()
        {
            GameObject bodyObject = Util.FindNetworkObject(this.netId);
            if (!bodyObject) return;

            RedGuyMod.Content.Components.RedGuyController penis = bodyObject.GetComponent<RedGuyMod.Content.Components.RedGuyController>();
            if (penis) penis.meter = this.fill * 0.01f;
        }

        public void Serialize(NetworkWriter writer)
        {
            writer.Write(this.netId);
            writer.Write(this.fill);
        }
    }
}
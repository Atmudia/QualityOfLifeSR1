
using MonomiPark.SlimeRancher.DataModel;
using SRML.SR.SaveSystem;
using System.IO;
using UnityEngine;

namespace QoLMod.Beacon
{
  public class BeaconGadgetModel : GadgetModel, ISerializableModel
  {
    public string BeaconName = "none";

    public BeaconGadgetModel(Gadget.Id ident, string siteId, Transform transform)
      : base(ident, siteId, transform)
    {
    }

    public void WriteData(BinaryWriter writer) => writer.Write(BeaconName);

    public void LoadData(BinaryReader reader) => BeaconName = reader.ReadString();
  }
}

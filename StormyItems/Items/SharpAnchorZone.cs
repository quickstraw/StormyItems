using RoR2;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Runtime.InteropServices;
using System.Text;
using UnityEngine;
using UnityEngine.Networking;

namespace StormyItems.Items
{
    [RequireComponent(typeof(TeamFilter))]
    class SharpAnchorZone : NetworkBehaviour
    {
		private void Awake()
		{
			teamFilter = GetComponent<TeamFilter>();
			rangeIndicator = transform.GetChild(0);
			RaycastHit raycastHit;
			if (NetworkServer.active && floorWard && Physics.Raycast(transform.position, Vector3.down, out raycastHit, 500f, LayerIndex.world.mask))
			{
				transform.position = raycastHit.point;
				transform.up = raycastHit.normal;
			}
		}

		private void Update()
		{
			if (this.rangeIndicator)
			{
				float num = Mathf.SmoothDamp(rangeIndicator.localScale.x, radius, ref rangeIndicatorScaleVelocity, 0.2f);
				this.rangeIndicator.localScale = new Vector3(num, num, num);
			}
		}

		private void FixedUpdate()
		{
			if (NetworkServer.active)
			{
				BuffOccupants();
			}
		}

		private void BuffOccupants()
		{
			ReadOnlyCollection<TeamComponent> teamMembers = TeamComponent.GetTeamMembers(teamFilter.teamIndex);
			float num = radius * radius;
			Vector3 position = transform.position;
			for (int i = 0; i < teamMembers.Count; i++)
			{
				if ((teamMembers[i].transform.position - position).sqrMagnitude <= num)
				{
					CharacterBody charComponent = teamMembers[i].GetComponent<CharacterBody>();
					if (charComponent && SharpAnchor.buffDef)
					{
						
						if (!charComponent.HasBuff(SharpAnchor.buffDef))
						{
							charComponent.MarkAllStatsDirty();
						}
						charComponent.AddTimedBuff(SharpAnchor.buffDef, 0.5f, GetHolderAnchorCount());
					}
				}
			}
		}

		private int GetHolderAnchorCount()
		{
			int count = 0;
			var zones = SharpAnchor.Zones;
			for(int i = 0; i < zones.Count; i++)
			{
				if(zones[i] == gameObject)
				{
					var charBody = Main.CharBodies[i];
					int index = 0;
					for (index = 0; index < Main.Items.Count; index++)
					{
						if(Main.Items[index].ItemLangTokenName == "SHARP_ANCHOR")
						{
							break;
						}
					}
					count = charBody.inventory.GetItemCount(Main.Items[index].ItemDef);
					break;
				}
			}

			return count;
		}

		private void UNetVersion()
		{
		}

		public float Networkradius
		{
			get
			{
				return radius;
			}
			[param: In]
			set
			{
				SetSyncVar<float>(value, ref radius, 1U);
			}
		}

		public override bool OnSerialize(NetworkWriter writer, bool forceAll)
		{
			if (forceAll)
			{
				writer.Write(radius);
				return true;
			}
			bool flag = false;
			if ((base.syncVarDirtyBits & 1U) != 0U)
			{
				if (!flag)
				{
					writer.WritePackedUInt32(syncVarDirtyBits);
					flag = true;
				}
				writer.Write(radius);
			}
			if (!flag)
			{
				writer.WritePackedUInt32(syncVarDirtyBits);
			}
			return flag;
		}

		public override void OnDeserialize(NetworkReader reader, bool initialState)
		{
			if (initialState)
			{
				radius = reader.ReadSingle();
				return;
			}
			int num = (int)reader.ReadPackedUInt32();
			if ((num & 1) != 0)
			{
				radius = reader.ReadSingle();
			}
		}

		public override void PreStartClient()
		{
		}

		[SyncVar]
		[Tooltip("The area of effect.")]
		public float radius;

		[Tooltip("The child range indicator object. Will be scaled to the radius.")]
		public Transform rangeIndicator;

		[Tooltip("Should the ward be floored on start")]
		public bool floorWard;

		private TeamFilter teamFilter;

		private float rangeIndicatorScaleVelocity;
	}
}

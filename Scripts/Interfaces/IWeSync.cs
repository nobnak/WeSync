using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace WeSyncSys {

	public interface IWeSync {
		WeSpace Space { get; }
		WeTime Time { get; }
		WeProjection Proj { get; }
	}
}

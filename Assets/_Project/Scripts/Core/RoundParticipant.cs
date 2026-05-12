using UnityEngine;

public class RoundParticipant : MonoBehaviour {
    [SerializeField] RoundRole role;

    public RoundRole Role {
        get { return role; }
    }

    public void SetRole (RoundRole newRole) {
        role = newRole;
    }
}

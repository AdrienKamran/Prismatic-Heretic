using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DialogTrigger : MonoBehaviour
{

	public Dialog dialogue;

    public void TriggerDialogue()
	{
		FindObjectOfType<DialogManager>().StartDialogue(dialogue);
		Destroy(this.gameObject);
	}

}

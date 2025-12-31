using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Interactable : MonoBehaviour
{
    // Poin 4: Base Class untuk GameObject Utama
    public string promptMessage = "Tekan E untuk Interaksi";

    public void BaseInteract()
    {
        Interact();
    }

    protected abstract void Interact(); // Fungsi kosong yang nanti diisi anak-anaknya
}

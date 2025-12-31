using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Script ini HARUS ditempel di objek Altar/Meja Kuis
// Pastikan objek tersebut memiliki komponen BoxCollider2D (Is Trigger = Centang)
// dan Layer yang sesuai agar bisa dideteksi oleh Player.

public class QuestionStation : Interactable
{
    [Header("Data Soal Spesifik")]
    public Sprite gambarSoal;      // Tarik file PNG soal ke sini lewat Inspector
    public string pilihanA;        // Teks untuk jawaban A
    public string pilihanB;        // Teks untuk jawaban B
    public bool jawabanBenarAdalahA; // Centang jika kunci jawabannya adalah A

    [Header("Data Jawaban")]
    public Sprite gambarPenjelasan;

    private bool sudahSelesai = false; // Penanda agar kuis tidak bisa diulang jika sudah benar

    // Fungsi ini dipanggil otomatis dari script Player saat menekan 'E'
    // (Karena mewarisi class Interactable)
    protected override void Interact()
    {
        // Cek apakah kuis ini sudah selesai? Jika belum, buka soal.
        if (!sudahSelesai)
        {
            // Kirim data (Gambar & Teks) ke QuizManager untuk ditampilkan di layar
            QuizManager.instance.BukaKuis(gambarSoal, pilihanA, pilihanB, jawabanBenarAdalahA, this);
        }
    }

    // Fungsi ini dipanggil balik oleh QuizManager jika jawaban Player BENAR
    public void MatikanAltar()
    {
        sudahSelesai = true;

        // Ubah warna altar menjadi gelap sebagai tanda sudah selesai
        GetComponent<SpriteRenderer>().color = Color.gray;

        // Matikan trigger agar tidak bisa interaksi lagi
        GetComponent<BoxCollider2D>().enabled = false;
    }
}

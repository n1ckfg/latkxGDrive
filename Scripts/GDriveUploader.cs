using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityGoogleDrive;

public class GDriveUploader : MonoBehaviour { 

    public LightningArtist latk;

    private string UploadFilePath;
    private GoogleDriveFiles.CreateRequest request;
    private string result;
    private bool armUploadFile = false;

    private void Update() {
        if (!armUploadFile && latk.isWritingFile) {
            armUploadFile = true;
        }

        if (armUploadFile && !latk.isWritingFile) {
            UploadFilePath = latk.url;
            StartCoroutine(doUpload());
            armUploadFile = false;
            if (request != null && request.IsRunning) {
                Debug.Log("Begin GDrive uploading...");
            }
        }
    }

    private IEnumerator doUpload() {
        Upload(false);

        yield return new WaitForSeconds(0f);

        Debug.Log("GDrive upload complete: " + result);
    }

    private void Upload(bool toAppData) {
        var content = File.ReadAllBytes(UploadFilePath);
        if (content == null) return;

        var file = new UnityGoogleDrive.Data.File() { Name = Path.GetFileName(UploadFilePath), Content = content };
        if (toAppData) file.Parents = new List<string> { "appDataFolder" };
        request = GoogleDriveFiles.Create(file);
        request.Fields = new List<string> { "id", "name", "size", "createdTime" };
        request.Send().OnDone += PrintResult;
    }

    private void PrintResult(UnityGoogleDrive.Data.File file) {
        result = string.Format("Name: {0} Size: {1:0.00}MB Created: {2:dd.MM.yyyy HH:MM:ss}\nID: {3}",
                file.Name,
                file.Size * .000001f,
                file.CreatedTime,
                file.Id);
    }

}

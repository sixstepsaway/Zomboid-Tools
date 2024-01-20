using Godot;
using ZomboidItemListTool.Containers;
using System;
using System.Diagnostics;
using System.Threading;

public partial class MainScene : MarginContainer
{

	[Signal]
	public delegate void CancelProcessingEventHandler();
	FileDialog ModsFolderDialog;
	FileDialog SaveFolderDialog;
	ProgressBar progressBar;
	TextEdit ModFolderLocation;
	TextEdit OutputFileLocation;
	Label pBarText;
	private string _inputfolder = "";
	public string inputfolder = "";
	private string _outputfolder = "";
	public string outputfolder = "";
	private int modcount = 0;
	private int modsread = 0;
	Button GenerateButton;
	Button CancelButton;

	Signaller signaller = new();
	Control SignallerControl;

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		SignallerControl = GetNode<Control>("Signaller");
		SignallerControl.Connect("ModsCountChanged", Callable.From(_on_signaller_mods_count_changed));
		SignallerControl.Connect("ModsReadChanged", Callable.From(_on_signaller_mods_read_changed));
		SignallerControl.Connect("ProcessingComplete", Callable.From(_on_processing_complete));
		ModsFolderDialog = GetNode<FileDialog>("FileDialog_ModsFolder");
		SaveFolderDialog = GetNode<FileDialog>("FileDialog_SaveFile");
		ModFolderLocation = GetNode<TextEdit>("MarginContainer/VBoxContainer/HBoxContainer/ModFolderLocationTXT");
		OutputFileLocation = GetNode<TextEdit>("MarginContainer/VBoxContainer/HBoxContainer2/OutputFileLocationTXT");
		progressBar = GetNode<ProgressBar>("MarginContainer/VBoxContainer/MarginContainer/ProgressBar");	
		pBarText = GetNode<Label>("MarginContainer/VBoxContainer/MarginContainer/Label");	
		progressBar.Value = 0;
		pBarText.Text = "Waiting to generate";
		GenerateButton = GetNode<Button>("MarginContainer/VBoxContainer/Generate");
		CancelButton = GetNode<Button>("MarginContainer/VBoxContainer/Cancel");
		GenerateButton.Visible = true;
		CancelButton.Visible = false;
	}

    private void _on_signaller_mods_count_changed(){
		modcount = SignallerControl.GetMeta("Modscount").AsInt32();
		progressBar.MaxValue = modcount;
	}

	private void _on_processing_complete(){
		GenerateButton.Visible = true;
		CancelButton.Visible = false;	
		pBarText.Text = "Processing Cancelled - Waiting";	
	}
	private void _on_signaller_mods_read_changed(){
		modsread = SignallerControl.GetMeta("Modsread").AsInt32();
		progressBar.Value = modsread;
		pBarText.Text = string.Format("Processed {0}/{1}", progressBar.Value, progressBar.MaxValue);
		if (modcount == modsread && modcount != 0 && modsread != 0){
			pBarText.Text = "Done!";
		}
	}

	private void _on_cancel_pressed(){
		EmitSignal("CancelProcessing");
	}

	private void _on_mod_folder_location_txt_text_changed(){
		inputfolder = ModFolderLocation.Text;
	}
	private void _on_mod_folder_location_txt_text_set(){
		inputfolder = ModFolderLocation.Text;
	}
	private void _on_output_file_location_txt_text_changed(){
		outputfolder = OutputFileLocation.Text;
	}

	private void _on_output_file_location_txt_text_set(){
		outputfolder = OutputFileLocation.Text;
	}

	private void _on_mod_folder_location_brwse_pressed(){
		ModsFolderDialog.Visible = true;
	}

	private void _on_output_file_location_brwse_pressed(){
		SaveFolderDialog.Visible = true;
	}
	private void _on_generate_pressed(){	
		if (inputfolder == "" || outputfolder == ""){
			GD.Print("pick locations dumbass");
			Error();
		} else {		
			GenerateButton.Visible = false;
			CancelButton.Visible = true;
			SignallerControl.Call("Generate", inputfolder, outputfolder);
		}	
	}

	private void Error(){
		string message = "";
		if (inputfolder == "" && outputfolder == ""){
			message = "Mods folder and output folder are both required.";
		} else if (inputfolder == ""){
			message = "Mods folder is required.";
		} else {
			message = "Output folder is required.";
		}
		AcceptDialog acceptDialog = GetNode<AcceptDialog>("AcceptDialog");
		acceptDialog.DialogText = message;
		acceptDialog.Visible = true;
	}
	private void _on_button_github_pressed(){
		Process.Start(new ProcessStartInfo("https://github.com/sixstepsaway/Zomboid-Tools") { UseShellExecute = true });		
	}
	private void _on_button_ko_fi_pressed(){
		Process.Start(new ProcessStartInfo("http://ko-fi.com/sixstepsaway") { UseShellExecute = true });
	}
	private void _on_file_dialog_mods_folder_dir_selected(string dir){
		_inputfolder = dir;
	}
	private void _on_file_dialog_mods_folder_canceled(){
		ModsFolderDialog.Visible = false;
	}
	private void _on_file_dialog_mods_folder_confirmed(){		
		ModsFolderDialog.Visible = false;
		inputfolder = _inputfolder;
		ModFolderLocation.Text = inputfolder;
	}
	private void _on_file_dialog_save_file_dir_selected(string dir){
		_outputfolder = dir;
	}
	private void _on_file_dialog_save_file_canceled(){
		SaveFolderDialog.Visible = false;
	}
	private void _on_file_dialog_save_file_confirmed(){			
		SaveFolderDialog.Visible = false;
		outputfolder = _outputfolder;
		OutputFileLocation.Text = outputfolder;
	}







	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{		
		
	}
}

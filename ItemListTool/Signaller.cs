using Godot;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;
using System.Xml.Serialization;
using Newtonsoft.Json;
using ZomboidItemListTool.Containers;
using System.Threading;

public partial class Signaller : Control
{

	[Signal]
	public delegate void ModsCountChangedEventHandler();
	[Signal]
	public delegate void ModsReadChangedEventHandler();
	[Signal]
	public delegate void ProcessingCompleteEventHandler();
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		MainNode = GetParent() as MarginContainer;
		MainNode.Connect("CancelProcessing", Callable.From(_on_processing_cancelled));
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	}
	private MarginContainer MainNode;
	private int _modscount;
	private int _modsread;

	public bool cancelled = false;

	public int ModsCount {
		get {return _modscount;}
		set {_modscount = value; 
		SetMeta("Modscount", value);
		EmitSignal("ModsCountChanged");}
	}
	public int ModsRead{
		get {return _modsread;}
		set {_modsread = value;
		SetMeta("Modsread", value);
		EmitSignal("ModsReadChanged");}
	}

	public void _on_processing_cancelled(){
		cancelled = true;
	}

	public void EmitComplete(){
		EmitSignal("ProcessingComplete");
	}

	public void IncrementModsRead(){
		ModsRead++;		
	}
	
	public void Generate(string inputfolder, string outputfolder){ 
		ModsRead = 0;		
		ZomboidItemsList ItemList = new();
		ZomboidItem newItem = new();
		List<string> ModFolders = Directory.GetDirectories(inputfolder).ToList();
		ModsCount = ModFolders.Count;
		new Thread (() => {
			foreach (string folder in ModFolders){   
				if (cancelled){
					CallDeferred("EmitComplete");
					break;
				}              
				DirectoryInfo workshopfolder = new(folder);    
				//GD.Print(string.Format("Processing folder: {0}", workshopfolder.Name));       
				List<string> modnamefolders = Directory.GetDirectories(Path.Combine(folder, "mods")).ToList();                
				foreach (string modnamefolder in modnamefolders){
					DirectoryInfo modname = new(modnamefolder);
					//GD.Print(string.Format("Processing mod folder: {0}", modname.Name));
					string mediafolder = Path.Combine(modnamefolder, "media");
					string scriptsfolder = Path.Combine(mediafolder, "scripts");
					string currentmod = modname.Name;
					if (Directory.Exists(scriptsfolder)){
						//GD.Print(string.Format("Found scripts folder."));
						List<string> scripts = Directory.EnumerateFiles(scriptsfolder, "*.txt", SearchOption.AllDirectories).ToList();
						if (scripts.Count != 0){
							//GD.Print(string.Format("Found non-zero number of files in scripts ({0})", scripts.Count));
							foreach (string script in scripts){                                
								FileInfo fileInfo = new(script);
								//GD.Print(string.Format("Found Items File: {0}", fileInfo.Name));
								using (FileStream fileStream = new(script, FileMode.Open, System.IO.FileAccess.Read)){
									using (StreamReader streamReader = new(fileStream)){
										bool eos = false;
										bool item = false;
										string module = "";
										while (eos == false){
											if (!streamReader.EndOfStream){
												string line = streamReader.ReadLine();  
												line = line.TrimStart();
												if (line.StartsWith("item ")){
													item = true;
													string itemname = line.Replace("item ", "");
													if (itemname.Contains("/*")){
														string[] split = itemname.Split("/*");
														itemname = split[0].TrimEnd().TrimStart();
													}
													newItem = new(){ Name = itemname, Module = module};
													//GD.Print(string.Format("Found Item: {0}", newItem.Name));
												} else if (line.StartsWith("module")){
													module = line.Replace("module ", "");
												} else if (item){
													if (line.Contains('}')){
														item = false;
														//GD.Print(string.Format("Item {0} completed.", newItem.Name));
														var exists = ItemList.Mods.Where(x => x.Name == modname.Name).ToList();
														if (exists.Any()){
															int idx = ItemList.Mods.IndexOf(exists.First());
															ItemList.Mods[idx].Items.Add(newItem);
														} else {
															ZomboidMod zomboidMod = new(){
																WorkshopID = workshopfolder.Name, Name = modname.Name
															};
															zomboidMod.Items.Add(newItem);
															ItemList.Mods.Add(zomboidMod);
														}
													} else if (line.Contains('=')){
														List<string> splitline = line.Split("=").ToList();
														string key = splitline[0].TrimEnd();
														key = key.TrimStart();
														string value = splitline[1].Replace(",", "");
														value = value.TrimEnd();
														value = value.TrimStart();
														//GD.Print(string.Format("Adding data for item: {0} - {1} - {2}", newItem.Name, key, value));
														newItem.Stats.Add(new ZomboidStat(){ Stat = key, Value = value});
														//GD.Print(string.Format("Data added."));
													}
												}
											} else {
												eos = true;
											}                                              
										}
										streamReader.Close();
									}
									fileStream.Close();
								}
							} 
						}
						
					}
					
				}
				CallDeferred("IncrementModsRead");
			}

			string outputfileXML = Path.Combine(outputfolder, "ZomboidOutput.xml");
			string outputfileJSON = Path.Combine(outputfolder, "ZomboidOutput.json");
			outputfileJSON = CheckExists(outputfileJSON);
			outputfileXML = CheckExists(outputfileXML);
			


			XmlSerializer zedSerializer = new XmlSerializer(typeof(ZomboidItemsList)); 
			using (var writer = new StreamWriter(outputfileXML))
			{
				zedSerializer.Serialize(writer, ItemList);
			}
			string jsonString = JsonConvert.SerializeObject(ItemList, Formatting.Indented);
			File.WriteAllText(outputfileJSON, jsonString);
		}){IsBackground = true}.Start();
		EmitSignal("ProcessingComplete");
	}

	private string CheckExists(string input, int adjustment = 0, string filename = ""){
		string newname = "";
		FileInfo file = new(input);
		if (filename == ""){
			filename = file.Name.Replace(file.Extension, "");
		}		
		string loc = file.Directory.FullName;
		if (File.Exists(input)){
			adjustment++;
			newname = Path.Combine(loc, string.Format("{0} ({1}){2}", filename, adjustment, file.Extension));
			newname = CheckExists(newname, adjustment, filename);
			return newname;
		} else {
			return input;
		}
		
	}
}

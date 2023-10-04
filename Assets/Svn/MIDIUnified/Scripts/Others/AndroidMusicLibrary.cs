using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Song
{
	private string filePath;

	public string title { get; private set; }
	public string artist { get; private set; }
	public string album { get; private set; }

	public Song(string filePath, string title, string artist, string album)
	{
		this.filePath = filePath;
		this.title = title;
		this.artist = artist;
		this.album = album;
	}

	public byte[] GetBytes()
	{
		// maybe its necessary to use WWW class here...
		return System.IO.File.ReadAllBytes(filePath);
	}
}

public class Source
{
	public string name { get; private set; }

	private List<Song> songs;

	public Source(string name, List<Song> songs)
	{
		this.name = name;
		this.songs = songs;
	}

	public List<Song> GetSongs()
	{
		return this.songs;
	}
}


public static class MusicLibraryPlugin
{

	// https://stackoverflow.com/questions/13568798/list-all-music-in-mediastore-with-the-paths
	public static List<Source> GetSources()
	{
		var songs = new List<Song>();

		var srcList = new string[] { "EXTERNAL_CONTENT_URI", "INTERNAL_CONTENT_URI" };

		using (AndroidJavaClass javaUnityPlayer = new AndroidJavaClass("com.unity3d.player.UnityPlayer"))
		{
			using (var currentActivity = javaUnityPlayer.GetStatic<AndroidJavaObject>("currentActivity"))
			{

				//ContentResolver cr = getActivity().getContentResolver();
				var cr = currentActivity.Call<AndroidJavaObject>("getContentResolver");

				foreach (var uriSource in srcList)
				{
					string uri;
					string selection;
					string titleStr;
					string artistStr;
					string albumStr;
					string dataStr;

					using (var media = new AndroidJavaClass("MediaStore.Audio.Media"))
					{
						uri = media.GetStatic<string>(uriSource);
						selection = media.GetStatic<string>("IS_MUSIC") + " != 0";
						titleStr = media.GetStatic<string>("TITLE");
						artistStr = media.GetStatic<string>("ARTIST");
						albumStr = media.GetStatic<string>("ALBUM");
						dataStr = media.GetStatic<string>("DATA");
					}

					var cur = cr.Call<AndroidJavaObject>("query", new object[] { uri, null, selection, null, null });

					int count = 0;
					if (cur != null)
					{
						count = cur.Call<int>("getCount");
						if (count > 0)
						{
							while (cur.Call<bool>("moveToNext"))
							{

								int titleColumn = cur.Call<int>("getColumnIndex", new object[] { titleStr });
								// int artistColumn = cur.Call<int>("getColumnIndex", new object[] { artistStr });
								int albumColumn = cur.Call<int>("getColumnIndex", new object[] { albumStr });
								var dataColumn = cur.Call<int>("getColumnIndex", dataStr);

								var filePath = cur.Call<string>("getString", dataColumn);
								var title = cur.Call<string>("getString", titleColumn);
								var artist = cur.Call<string>("getString", artistStr);
								var album = cur.Call<string>("getString", albumColumn);

								var song = new Song(filePath, title, artist, album);
								songs.Add(song);
							}
						}
					}

				}

			}

			var source = new Source("Media Library", songs);
			return new List<Source>() { source };
		}
	}

}

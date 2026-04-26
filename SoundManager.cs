using System;
using System.IO;
using System.Windows.Forms;
using WMPLib;

namespace BattleOfEntities
{
    public static class SoundManager
    {
        private static WindowsMediaPlayer _backgroundPlayer = new WindowsMediaPlayer();
        private static string _currentMusic = "";

        public static void PlayBackgroundMusic(string fileName)
        {
            try
            {
                string musicPath = Path.Combine(Application.StartupPath, "Sounds", fileName);

                if (File.Exists(musicPath))
                {
                    // Если эта же музыка уже играет - не перезапускаем
                    if (_currentMusic == musicPath)
                        return;

                    _currentMusic = musicPath;
                    _backgroundPlayer.URL = musicPath;
                    _backgroundPlayer.settings.setMode("loop", true);
                    _backgroundPlayer.settings.volume = 50;
                    _backgroundPlayer.controls.play();
                }
                else
                {
                    MessageBox.Show($"Файл не найден: {musicPath}", "Ошибка музыки");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка воспроизведения музыки: {ex.Message}", "Ошибка");
            }
        }

        public static void StopBackgroundMusic()
        {
            _backgroundPlayer.controls.stop();
            _currentMusic = "";
        }

        public static void SetVolume(int volume)
        {
            _backgroundPlayer.settings.volume = volume;
        }

        // Воспроизведение звука без зацикливания (для побед/поражений)
        public static void PlayOneShot(string fileName)
        {
            try
            {
                string soundPath = Path.Combine(Application.StartupPath, "Sounds", fileName);

                if (File.Exists(soundPath))
                {
                    var tempPlayer = new WindowsMediaPlayer();
                    tempPlayer.URL = soundPath;
                    tempPlayer.settings.volume = 70;
                    tempPlayer.controls.play();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Ошибка воспроизведения звука: {ex.Message}");
            }
        }
    }
}
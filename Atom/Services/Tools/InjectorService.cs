using System;
using System.Diagnostics;
using System.IO;
using System.Runtime.InteropServices;
using System.Text;
using Atom.Utils;

namespace Atom.Services.Tools
{
    public static class InjectorService
    {
        [DllImport("kernel32.dll", SetLastError = true)]
        static extern IntPtr OpenProcess(uint dwDesiredAccess, bool bInheritHandle, int dwProcessId);

        [DllImport("kernel32.dll", SetLastError = true)]
        static extern IntPtr GetModuleHandle(string lpModuleName);

        [DllImport("kernel32.dll", SetLastError = true)]
        static extern IntPtr GetProcAddress(IntPtr hModule, string lpProcName);

        [DllImport("kernel32.dll", SetLastError = true)]
        static extern IntPtr VirtualAllocEx(IntPtr hProcess, IntPtr lpAddress, uint dwSize, uint flAllocationType, uint flProtect);

        [DllImport("kernel32.dll", SetLastError = true)]
        static extern bool WriteProcessMemory(IntPtr hProcess, IntPtr lpBaseAddress, byte[] lpBuffer, uint nSize, out IntPtr lpNumberOfBytesWritten);

        [DllImport("kernel32.dll", SetLastError = true)]
        static extern IntPtr CreateRemoteThread(IntPtr hProcess, IntPtr lpThreadAttributes, uint dwStackSize, IntPtr lpStartAddress, IntPtr lpParameter, uint dwCreationFlags, out IntPtr lpThreadId);

        [DllImport("kernel32.dll", SetLastError = true)]
        static extern bool CloseHandle(IntPtr hObject);

        // Access rights
        const uint PROCESS_ALL_ACCESS = 0x001F0FFF;
        const uint MEM_COMMIT = 0x00001000;
        const uint MEM_RESERVE = 0x00002000;
        const uint PAGE_READWRITE = 0x04;

        public static void HandleInjectorMenu()
        {
            UIHelper.PrintSectionHeader("DLL Injector");

            string procName = UIHelper.Prompt("Nom du processus (ex: notepad)") ?? "";
            if (string.IsNullOrEmpty(procName)) return;

            if (procName.EndsWith(".exe")) procName = procName.Substring(0, procName.Length - 4);

            Process[] processes = Process.GetProcessesByName(procName);
            if (processes.Length == 0)
            {
                UIHelper.PrintError($"Processus '{procName}' non trouvé.");
                return;
            }

            Process targetProcess = processes[0];
            UIHelper.PrintInfo($"Processus cible : {targetProcess.ProcessName} (PID: {targetProcess.Id})");

            string dllPath = UIHelper.Prompt("Chemin complet vers la DLL") ?? "";
            if (string.IsNullOrEmpty(dllPath)) return;

            if (!File.Exists(dllPath))
            {
                UIHelper.PrintError("Le fichier DLL n'existe pas.");
                return;
            }

            Inject(targetProcess.Id, dllPath);
        }

        private static void Inject(int pid, string dllPath)
        {
            IntPtr hProcess = OpenProcess(PROCESS_ALL_ACCESS, false, pid);
            if (hProcess == IntPtr.Zero)
            {
                UIHelper.PrintError("Impossible d'ouvrir le processus (Erreur: " + Marshal.GetLastWin32Error() + ")");
                return;
            }

            try
            {
                IntPtr loadLibAddr = GetProcAddress(GetModuleHandle("kernel32.dll"), "LoadLibraryA");
                if (loadLibAddr == IntPtr.Zero)
                {
                    UIHelper.PrintError("Impossible de trouver LoadLibraryA.");
                    return;
                }

                uint size = (uint)((dllPath.Length + 1) * Marshal.SizeOf(typeof(char)));
                IntPtr allocMemAddress = VirtualAllocEx(hProcess, IntPtr.Zero, size, MEM_COMMIT | MEM_RESERVE, PAGE_READWRITE);

                if (allocMemAddress == IntPtr.Zero)
                {
                    UIHelper.PrintError("Échec de l'allocation mémoire dans le processus cible.");
                    return;
                }

                byte[] buffer = Encoding.Default.GetBytes(dllPath);
                if (!WriteProcessMemory(hProcess, allocMemAddress, buffer, (uint)buffer.Length, out _))
                {
                    UIHelper.PrintError("Échec de l'écriture de la mémoire.");
                    return;
                }

                IntPtr threadHandle = CreateRemoteThread(hProcess, IntPtr.Zero, 0, loadLibAddr, allocMemAddress, 0, out _);
                if (threadHandle == IntPtr.Zero)
                {
                    UIHelper.PrintError("Échec de la création du thread distant.");
                    return;
                }

                UIHelper.PrintSuccess("Injection réussie !");
                CloseHandle(threadHandle);
            }
            catch (Exception ex)
            {
                UIHelper.PrintError($"Erreur lors de l'injection: {ex.Message}");
            }
            finally
            {
                CloseHandle(hProcess);
            }
        }
    }
}

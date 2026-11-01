using System.Text.Json;

namespace PCLD;

class Program
{
    private static readonly HttpClient _httpClient = new HttpClient
    {
        Timeout = TimeSpan.FromSeconds(30)
    };
    static async Task Main(string[] args)
    {
        Console.WriteLine("text测试0123456789abcdefghijklmnopqrstuvwxyz");
        await Task.Delay(1500);
        Console.Clear();
        Console.WriteLine("\r\n " +
            "██████╗  ██████╗██╗     ██████╗ \r\n" +
            "██╔══██╗██╔════╝██║     ██╔══██╗\r\n" +
            "██████╔╝██║     ██║     ██║  ██║\r\n" +
            "██╔═══╝ ██║     ██║     ██║  ██║\r\n" +
            "██║     ╚██████╗███████╗██████╔╝\r\n" +
            "╚═╝      ╚═════╝╚══════╝╚═════╝ \r\n                                ");
        Console.WriteLine("欢迎使用PCLD\r\n\r\n");
        await Task.Delay(1000);
        Console.WriteLine("免责声明：本程序仅提供Mojang公开api的版本Json文件下载器，完全免费，开源\r\n请勿相信任何第三方下载渠道，程序仅分发于github，由第三方修改、重新打包的 PCLD 产生的安全问题与原作者无关。\r\n\"Minecraft\"以及\"我的世界\"为美国微软公司的商标 开发者与微软公司没有任何隶属关系！\r\n");
        await Task.Delay(4500);
        Console.Clear();
        Console.WriteLine("加载中...");

        while (true)
        {
            await Task.Delay(5000);
            Console.Clear();
            Console.WriteLine("\r\n " +
            "██████╗  ██████╗██╗     ██████╗ \r\n" +
            "██╔══██╗██╔════╝██║     ██╔══██╗\r\n" +
            "██████╔╝██║     ██║     ██║  ██║\r\n" +
            "██╔═══╝ ██║     ██║     ██║  ██║\r\n" +
            "██║     ╚██████╗███████╗██████╔╝\r\n" +
            "╚═╝      ╚═════╝╚══════╝╚═════╝ ——MC_XYR\r\n" +
            "                                ");
            Console.Write("请输入要下载的版本号(官方命名规则；输入Q退出)：");
            string Version = Console.ReadLine() ?? "";

            if (Version.Equals("q", StringComparison.OrdinalIgnoreCase))
            {
                Console.WriteLine("已退出。");
                break;                    // ← 正常退出,退出码 0
            }

            if (string.IsNullOrWhiteSpace(Version))
            {
                Console.WriteLine("版本号为空,已退出。\r\n");
                Console.Write("请输入要下载的版本号(官方命名规则；输入Q退出)：");
            }

            try
            {
                //获取版本清单
                const string manifestUrl = "https://piston-meta.mojang.com/mc/game/version_manifest_v2.json";
                string manifestJson = await _httpClient.GetStringAsync(manifestUrl);

                using var doc = JsonDocument.Parse(manifestJson);
                var versions = doc.RootElement.GetProperty("versions");

                //找版本
                string? targetUrl = null;
                foreach (var v in versions.EnumerateArray())
                {
                    if (v.GetProperty("id").GetString() == Version)
                    {
                        targetUrl = v.GetProperty("url").GetString();
                        break;
                    }
                }

                if (targetUrl is null)
                {
                    Console.WriteLine($"未找到版本号:{Version}\r\n");
                    Console.Write("请输入要下载的版本号(官方命名规则；输入Q退出)：");
                }

                Console.WriteLine($"已找到 {Version},下载地址:{targetUrl}\r\n");

                //下载该 JSON
                string versionJson = await _httpClient.GetStringAsync(targetUrl);

                //保存到本地
                string savePath = Path.Combine(AppContext.BaseDirectory, $"{Version}.json");
                await File.WriteAllTextAsync(savePath, versionJson);

                Console.WriteLine($"下载完成,已保存到:{savePath} \r\n");
            }
            catch (HttpRequestException ex)
            {
                Console.WriteLine($"网络错误:{ex.Message}\r\n");
            }
            catch (JsonException ex)
            {
                Console.WriteLine($"JSON 解析错误:{ex.Message}\r\n");
            }
            catch (KeyNotFoundException ex)
            {
                Console.WriteLine($"数据字段缺失:{ex.Message}\r\n");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"发生错误:{ex.Message}\r\n");
            }
        }
    }
}
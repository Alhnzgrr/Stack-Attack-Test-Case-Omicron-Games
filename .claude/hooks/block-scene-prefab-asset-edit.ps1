$raw = [Console]::In.ReadToEnd()
try { $data = $raw | ConvertFrom-Json } catch { exit 0 }
$path = "$($data.tool_input.file_path)"
if ($path -match '\.(unity|prefab|asset)$') {
    [Console]::Error.WriteLine("Blocked: direct text edits to .unity/.prefab/.asset files are forbidden. Ask the user to do it in the Unity Editor.")
    exit 2
}
exit 0

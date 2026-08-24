$raw = [Console]::In.ReadToEnd()
try { $data = $raw | ConvertFrom-Json } catch { exit 0 }
$content = "$($data.tool_input.content)$($data.tool_input.new_string)"
if ($content -match '#region|#endregion') {
    [Console]::Error.WriteLine("Blocked: #region/#endregion is forbidden by this project's code style. Remove the region directives.")
    exit 2
}
exit 0

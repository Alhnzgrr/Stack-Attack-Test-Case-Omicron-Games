$raw = [Console]::In.ReadToEnd()
try { $data = $raw | ConvertFrom-Json } catch { exit 0 }
$content = "$($data.tool_input.content)$($data.tool_input.new_string)"
if ($content -match 'static\s+\w+\s+Instance\b') {
    [Console]::Error.WriteLine("Blocked: singleton pattern (static Instance) is forbidden. Use VContainer dependency injection.")
    exit 2
}
exit 0

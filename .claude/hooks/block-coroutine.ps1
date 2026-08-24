$raw = [Console]::In.ReadToEnd()
try { $data = $raw | ConvertFrom-Json } catch { exit 0 }
$content = "$($data.tool_input.content)$($data.tool_input.new_string)"
if ($content -match 'StartCoroutine\s*\(|IEnumerator\s+[A-Za-z_]\w*\s*\(') {
    [Console]::Error.WriteLine("Blocked: coroutines are forbidden. Use UniTask for async work.")
    exit 2
}
exit 0

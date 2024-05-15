<?php
require 'lib.php';
$key = $_GET["key"];
$filename = sanitise($key);
$path = 'data/' . $filename;

header_remove('Content-Type');
header('Content-Length', filesize($path));
header('Accept-Ranges', 'bytes');
header('Connection', 'close');
header('Content-Encoding', 'gz');
cors();

$FILE = fopen($path, 'rb');
$DATA = fread($FILE, filesize($path));
fclose($FILE);
echo $DATA;

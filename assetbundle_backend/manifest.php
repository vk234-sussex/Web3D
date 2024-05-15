<?php
require 'lib.php';
cors();

$FILE = fopen('manifest.json', 'r');
$DATA = fread($FILE, filesize('manifest.json'));
fclose($FILE);
header('Content-Type: application/json; charset=utf-8');
echo $DATA;

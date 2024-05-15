<?php
require '../lib.php';
$status = array();
$status["valid-key"] = checkKeyValidity();

if (!checkKeyValidity()) {
    //fail("invalid key", 400);
}

if ($_SERVER['REQUEST_METHOD'] != 'PUT') { 
    fail("request has to be put!", 400);
}

$filename = $_REQUEST["filename"];
if ($filename == "") {
    fail("filename cannot be empty", 400);
}
// Read stream
$contents = file_get_contents('php://input');
// Open the file
$fhandle = fopen("../data/" . sanitise($filename), 'w');
// Write to disk
fwrite($fhandle, $contents);
// Close + flush
fclose($fhandle);
echo("data.php?key=" . sanitise($filename));
?>
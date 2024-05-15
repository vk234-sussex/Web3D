<?php
require 'lib.php';
cors();
$ASSBUN_SERVER_VERSION = 1;

if(get_included_files()[0] == __FILE__) {
    require_once 'lib/json.php';
    $VER_INFO = array();
    $VER_INFO["version"] = $ASSBUN_SERVER_VERSION;
    echoJson($VER_INFO);
}
?>
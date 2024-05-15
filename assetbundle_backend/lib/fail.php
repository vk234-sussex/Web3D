<?php
function fail($failMsg, $failCode) {
    echo($failMsg);
    http_response_code($failCode);
    exit(1);
}
?>
<?php
// Use AIO's own container definition. No vendor files or application data are edited.
declare(strict_types=1);
require '/var/www/docker-aio/php/vendor/autoload.php';
$di = \AIO\DependencyInjection::GetContainer();
$config = $di->get(\AIO\Data\ConfigurationManager::class);
if (($argv[1] ?? '') === 'restore') {
    $config->apachePort = '443';
    $config->apacheIpBinding = '';
    echo "Original AIO integrated HTTPS configuration restored\n";
    exit(0);
}
if ($config->apachePort !== '11000' || $config->apacheIpBinding !== '127.0.0.1') {
    throw new RuntimeException('Unexpected AIO reverse proxy configuration');
}
if (($argv[1] ?? '') === 'check') {
    echo "AIO reverse proxy configuration verified\n";
    exit(0);
}
if (($argv[1] ?? '') !== 'create') {
    throw new RuntimeException('Expected check or create');
}
$fetcher = $di->get(\AIO\ContainerDefinitionFetcher::class);
$actions = $di->get(\AIO\Docker\DockerActionManager::class);
$apache = $fetcher->GetContainerById('nextcloud-aio-apache');
$actions->CreateVolumes($apache);
$actions->CreateContainer($apache);
$actions->StartContainer($apache);
$actions->ConnectContainerToNetwork($apache);
echo "AIO Apache recreated for the external proxy\n";

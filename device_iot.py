from azure.iot.device import IoTHubDeviceClient, Message
import json
import time

connection_string = "HostName=dev-iot-test.azure-devices.net;DeviceId=dv-iot-test;SharedAccessKey=yegh3uMo6QcXfjW2yIhigwJE2X70/XHRk5f+z1h45hQ="
device_id = "dv-iot-test"
topic = "devices/dv-iot-test/messages/events/".format(device_id)

def send_message(device_client):
    data = {
        "temperature": 25.5,
        "humidity": 60.2
    }

    message = Message(json.dumps(data))

    message.content_type = "application/json"
    message.content_encoding = "utf-8"

    device_client.send_message(message)
    print("Mensaje enviado: ", message)

def main():
    try:
        device_client = IoTHubDeviceClient.create_from_connection_string(connection_string)
        device_client.connect()

        while True:
            send_message(device_client)
            time.sleep(5)
    except KeyboardInterrupt:
        print("Detenido por el usuario")
    finally:
        device_client.disconnect()

if __name__ == "__main__":
    main()
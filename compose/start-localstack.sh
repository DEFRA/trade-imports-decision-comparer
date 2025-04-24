#!/bin/bash

export AWS_ENDPOINT_URL=http://localhost:4566
export AWS_REGION=eu-west-2
export AWS_DEFAULT_REGION=eu-west-2
export AWS_ACCESS_KEY_ID=test
export AWS_SECRET_ACCESS_KEY=test

aws --endpoint-url=http://localhost:4566 sqs create-queue \
    --queue-name trade_imports_data_upserted_decision_comparer

function is_ready() {
    aws --endpoint-url=http://localhost:4566 sqs get-queue-url --queue-name trade_imports_data_upserted_decision_comparer || return 1
    return 0
}

while ! is_ready; do
    echo "Waiting until ready"
    sleep 1
done

touch /tmp/ready
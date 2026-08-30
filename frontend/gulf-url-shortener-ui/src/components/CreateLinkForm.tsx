import { PlusOutlined } from "@ant-design/icons";
import { Button, Card, Col, Form, Input, Row, Space } from "antd";
import type { CreateLinkValues } from "../types/link";

type Props = {
  loading: boolean;
  onSubmit: (values: CreateLinkValues) => Promise<void>;
};

export function CreateLinkForm({ loading, onSubmit }: Props) {
  const [form] = Form.useForm<CreateLinkValues>();
  const submit = async (values: CreateLinkValues) => {
    await onSubmit(values);
    form.resetFields();
  };

  return (
    <Card
      className="create-card"
      title={
        <Space>
          <PlusOutlined /> Create a short link
        </Space>
      }
    >
      <Form form={form} layout="vertical" onFinish={submit}>
        <Row gutter={16}>
          <Col xs={24} md={16}>
            <Form.Item
              name="url"
              label="Long URL"
              rules={[
                {
                  required: true,
                  type: "url",
                  message: "Enter a valid URL including https://",
                },
              ]}
            >
              <Input
                size="large"
                placeholder="https://example.com/my-long-page"
              />
            </Form.Item>
          </Col>
          <Col xs={24} md={8}>
            <Form.Item
              name="alias"
              label="Custom alias (optional)"
              rules={[
                {
                  pattern: /^[A-Za-z0-9_-]{3,32}$/,
                  message: "Use 3-32 letters, numbers, - or _",
                },
              ]}
            >
              <Input size="large" placeholder="my-link" />
            </Form.Item>
          </Col>
        </Row>
        <Row gutter={16}>
          <Col xs={24} md={8}>
            <Form.Item name="defaultUrl" label="Default destination">
              <Input placeholder="Optional fallback URL" />
            </Form.Item>
          </Col>
          <Col xs={24} md={8}>
            <Form.Item name="iosUrl" label="iOS destination">
              <Input placeholder="Optional iPhone/iPad URL" />
            </Form.Item>
          </Col>
          <Col xs={24} md={8}>
            <Form.Item name="androidUrl" label="Android destination">
              <Input placeholder="Optional Android URL" />
            </Form.Item>
          </Col>
        </Row>
        <Button
          type="primary"
          size="large"
          htmlType="submit"
          loading={loading}
          icon={<PlusOutlined />}
        >
          Shorten URL
        </Button>
      </Form>
    </Card>
  );
}

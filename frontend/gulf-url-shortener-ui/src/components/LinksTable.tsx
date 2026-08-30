import { CopyOutlined, DeleteOutlined, StopOutlined } from "@ant-design/icons";
import {
  Button,
  Card,
  Popconfirm,
  Space,
  Statistic,
  Table,
  Tag,
  Tooltip,
  Typography,
} from "antd";
import type { ColumnsType } from "antd/es/table";
import type { LinkItem } from "../types/link";

type Props = {
  links: LinkItem[];
  loading: boolean;
  onDisable: (code: string) => Promise<void>;
  onDelete: (code: string) => Promise<void>;
  onMessage: (text: string) => void;
};

export function LinksTable({
  links,
  loading,
  onDisable,
  onDelete,
  onMessage,
}: Props) {
  const copy = async (url: string) => {
    try {
      await navigator.clipboard.writeText(url);
      onMessage("Copied to clipboard");
    } catch {
      onMessage("Could not copy link. Please copy it manually.");
    }
  };
  const columns: ColumnsType<LinkItem> = [
    {
      title: "Short link",
      dataIndex: "shortUrl",
      width: 245,
      render: (url: string, item) => (
        <Space direction="vertical" size={0}>
          <Tooltip title={url}>
            <Typography.Link
              ellipsis
              href={url}
              target="_blank"
            >
              {url}
            </Typography.Link>
          </Tooltip>
          <Typography.Text type="secondary">
            {item.isCustomAlias ? "Custom alias" : "Generated code"}:{" "}
            {item.code}
          </Typography.Text>
        </Space>
      ),
    },
    {
      title: "Destination",
      dataIndex: "originalUrl",
      width: 235,
      ellipsis: true,
      render: (url: string) => (
        <Typography.Text ellipsis={{ tooltip: url }}>{url}</Typography.Text>
      ),
    },
    {
      title: "Clicks",
      dataIndex: "clicks",
      width: 90,
      sorter: (a, b) => a.clicks - b.clicks,
      render: (clicks: number) => (
        <Statistic value={clicks} valueStyle={{ fontSize: 18 }} />
      ),
    },
    {
      title: "Status",
      dataIndex: "isDisabled",
      width: 105,
      render: (disabled: boolean) => (
        <Tag color={disabled ? "default" : "green"}>
          {disabled ? "Disabled" : "Active"}
        </Tag>
      ),
    },
    {
      title: "Created",
      dataIndex: "createdAt",
      width: 115,
      render: (date: string) => new Date(date).toLocaleDateString(),
    },
    {
      title: "Last accessed",
      dataIndex: "lastAccessedAt",
      width: 145,
      render: (date?: string) =>
        date ? (
          new Date(date).toLocaleString()
        ) : (
          <Typography.Text type="secondary">Never</Typography.Text>
        ),
    },
    {
      title: "Actions",
      key: "actions",
      fixed: "right",
      width: 245,
      render: (_value, item) => (
        <Space className="actions-cell" size={6} wrap>
          <Tooltip title="Copy short URL">
            <Button
              size="small"
              icon={<CopyOutlined />}
              onClick={() => void copy(item.shortUrl)}
            >
              Copy
            </Button>
          </Tooltip>
          {!item.isDisabled && (
            <Tooltip title="Stop this link from redirecting">
              <Button
                size="small"
                icon={<StopOutlined />}
                onClick={() => void onDisable(item.code)}
              >
                Disable
              </Button>
            </Tooltip>
          )}
          <Popconfirm
            title="Delete this link?"
            onConfirm={() => void onDelete(item.code)}
          >
            <Button size="small" danger icon={<DeleteOutlined />}>
              Delete
            </Button>
          </Popconfirm>
        </Space>
      ),
    },
  ];

  return (
    <Card
      className="links-card"
      title="Your links"
      extra={<Tag color="blue">{links.length} total</Tag>}
    >
      <Table
        rowKey="code"
        columns={columns}
        dataSource={links}
        loading={loading}
        pagination={{ pageSize: 6 }}
        scroll={{ x: 1180 }}
        locale={{ emptyText: "Create your first short link above" }}
      />
    </Card>
  );
}

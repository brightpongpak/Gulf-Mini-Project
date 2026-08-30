import { LinkOutlined } from "@ant-design/icons";
import { Layout, Typography } from "antd";

export function AppHeader() {
  return (
    <Layout.Header className="app-header">
      <div className="brand">
        <LinkOutlined />{" "}
        <span>
          Gulf<span className="brand-accent">Short</span>
        </span>
      </div>
      <Typography.Text className="header-caption">
        Simple links, clear insights
      </Typography.Text>
    </Layout.Header>
  );
}

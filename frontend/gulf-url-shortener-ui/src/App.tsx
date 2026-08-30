import { App as AntApp, Layout, Typography } from 'antd'
import './App.css'
import { AppHeader } from './components/AppHeader'
import { CreateLinkForm } from './components/CreateLinkForm'
import { LinksTable } from './components/LinksTable'
import { useDashboard } from './hooks/useDashboard'

function Dashboard() {
  const {
    links,
    loading,
    error,
    isCreating,
    handleCreate,
    handleDisable,
    handleDelete,
    notify,
    getErrorMessage,
  } = useDashboard()

  return (
    <Layout className="app-shell">
      <AppHeader />
      <Layout.Content className="content">
        <section className="hero">
          <Typography.Text className="eyebrow">URL LINK SHORTENER</Typography.Text>
          <Typography.Title>
            Turn long URLs into <span className="hero-accent">short, memorable links.</span>
          </Typography.Title>
          <Typography.Paragraph>
            Build branded links, route visitors by platform, and keep an eye on every click.
          </Typography.Paragraph>
        </section>

        {error && <Typography.Paragraph type="danger">{getErrorMessage(error, 'Could not load links')}</Typography.Paragraph>}
        <CreateLinkForm loading={isCreating} onSubmit={handleCreate} />
        <LinksTable links={links} loading={loading} onDisable={handleDisable} onDelete={handleDelete} onMessage={notify} />
      </Layout.Content>
      <Layout.Footer className="footer">GulfShort · Built with React, Ant Design, and ASP.NET Core</Layout.Footer>
    </Layout>
  )
}

export default function App() {
  return <AntApp><Dashboard /></AntApp>
}

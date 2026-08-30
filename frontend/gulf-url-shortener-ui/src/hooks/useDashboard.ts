import { useCallback } from 'react'
import { App as AntApp } from 'antd'
import { useLinks } from './useLinks'
import { getApiErrorMessage } from '../services/linkApi'
import type { CreateLinkValues } from '../types/link'

export function useDashboard() {
  const { message } = AntApp.useApp()
  const { links, loading, error, refresh, create, disable, remove, isCreating } = useLinks()

  const handleCreate = useCallback(async (values: CreateLinkValues) => {
    try {
      await create(values)
      message.success('Short link created')
    } catch (error) {
      message.error(getApiErrorMessage(error, 'Could not create link'))
      throw error
    }
  }, [create, message])

  const handleDisable = useCallback(async (code: string) => {
    try {
      await disable(code)
      message.success('Link disabled')
    } catch (error) {
      message.error(getApiErrorMessage(error, 'Could not disable link'))
    }
  }, [disable, message])

  const handleDelete = useCallback(async (code: string) => {
    try {
      await remove(code)
      message.success('Link deleted')
    } catch (error) {
      message.error(getApiErrorMessage(error, 'Could not delete link'))
    }
  }, [remove, message])

  const notify = useCallback((text: string) => message.success(text), [message])

  return { links, loading, error, refresh, isCreating, handleCreate, handleDisable, handleDelete, notify, getErrorMessage: getApiErrorMessage }
}
